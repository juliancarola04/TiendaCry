using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Repositories;
using TiendaCry.Shared.DTO.Auth.Response;

namespace TiendaCry.Backend.Services
{
    public class SesionService
    {
        private readonly ISesionUsuarioRepository _sesionRepository;
        private readonly ITokenService _tokenService;

        public SesionService(ISesionUsuarioRepository sesionRepository, ITokenService tokenService)
        {
            _sesionRepository = sesionRepository;
            _tokenService = tokenService;
        }

        public async Task Cerrar(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new DatosLlegaronErradosException("El refresh token es obligatorio.");
            }

            var codigo = _tokenService.CalcularHash(refreshToken);
            var sesion = await _sesionRepository.BuscarPorRefreshToken(codigo);

            if (sesion is null || sesion.FechaRevocacion is not null)
            {
                return;
            }

            sesion.FechaRevocacion = DateTime.UtcNow;

            try
            {
                await _sesionRepository.GuardarCambios();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new SesionInvalidaException();
            }
        }

        public async Task<AutenticacionResponse> Renovar(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new DatosLlegaronErradosException("El refresh token es obligatorio.");
            }

            var codigo = _tokenService.CalcularHash(refreshToken);
            var sesion = await _sesionRepository.BuscarPorRefreshToken(codigo);
            var ahora = DateTime.UtcNow;

            if (sesion is null || sesion.FechaRevocacion is not null || sesion.FechaVencimiento <= ahora || sesion.Usuario.EstadoUsuario.Nombre != "Activo")
            {
                throw new SesionInvalidaException();
            }

            var token = _tokenService.CrearToken(sesion.Usuario);
            var nuevoRefreshToken = _tokenService.CrearRefreshToken();

            sesion.RefreshToken = _tokenService.CalcularHash(nuevoRefreshToken);
            sesion.FechaVencimiento = ahora.AddDays(30);

            try
            {
                await _sesionRepository.GuardarCambios();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new SesionInvalidaException();
            }

            return new AutenticacionResponse(token, nuevoRefreshToken);
        }
    }
}
