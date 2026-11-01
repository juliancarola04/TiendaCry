using Microsoft.EntityFrameworkCore;
using TiendaCry.Backend.Excepciones;
using TiendaCry.Backend.Models;
using TiendaCry.Backend.Repositories;
using TiendaCry.Shared.DTO.Auth.Response;

namespace TiendaCry.Backend.Services
{
    public class SesionService
    {
        private readonly ILogger<SesionService> _logger;
        private readonly ISesionUsuarioRepository _sesionRepository;
        private readonly ITokenService _tokenService;
        private readonly IUsuarioRepository _usuarioRepository;

        public SesionService(ISesionUsuarioRepository sesionRepository, ITokenService tokenService, IUsuarioRepository usuarioRepository, ILogger<SesionService> logger)
        {
            _logger = logger;
            _sesionRepository = sesionRepository;
            _tokenService = tokenService;
            _usuarioRepository = usuarioRepository;
        }

        public async Task Cerrar(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new DatosLlegaronErradosException("El refresh token es obligatorio.");
            }

            string codigo = _tokenService.CalcularHash(refreshToken);
            SesionUsuario? sesion = await _sesionRepository.BuscarPorRefreshToken(codigo);

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

            _logger.LogInformation("El usuario {IdUsuario} cerró la sesión {IdSesion}.", sesion.IdUsuario, sesion.Id);
        }

        public async Task<AutenticacionResponse> Renovar(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new DatosLlegaronErradosException("El refresh token es obligatorio.");
            }

            string codigo = _tokenService.CalcularHash(refreshToken);
            SesionUsuario? sesion = await _sesionRepository.BuscarPorRefreshToken(codigo);
            DateTime ahora = DateTime.UtcNow;

            if (sesion is null || sesion.FechaRevocacion is not null || sesion.FechaVencimiento <= ahora || sesion.Usuario.EstadoUsuario.Nombre != "Activo")
            {
                throw new SesionInvalidaException();
            }

            List<string> roles = await _usuarioRepository.ObtenerRoles(sesion.IdUsuario);

            string token = _tokenService.CrearToken(sesion.Usuario, roles);
            string nuevoRefreshToken = _tokenService.CrearRefreshToken();

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

            _logger.LogDebug("Se renovó la sesión {IdSesion} del usuario {IdUsuario}.", sesion.Id, sesion.IdUsuario);

            return new AutenticacionResponse(token, nuevoRefreshToken);
        }
    }
}
