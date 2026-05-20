using Votify.Core.Interfaces;

namespace Votify.Services.Implementations.IA
{
    public class MockIAProvider : IIAProvider
    {
        public Task<string> AnalizarAsync(string prompt)
        {
            string respuesta = """
            {
                "sugerencias": [
                    {
                        "prioridad": 1,
                        "categoria": "Documentación",
                        "descripcion": "Mejorar la documentación del código",
                        "accionRecomendada": "Añadir comentarios XML y un README detallado"
                    },
                    {
                        "prioridad": 2,
                        "categoria": "Testing",
                        "descripcion": "Aumentar la cobertura de tests",
                        "accionRecomendada": "Añadir tests unitarios para los servicios principales"
                    },
                    {
                        "prioridad": 3,
                        "categoria": "Arquitectura",
                        "descripcion": "Aplicar principios SOLID",
                        "accionRecomendada": "Separar responsabilidades en clases más pequeñas"
                    }
                ]
            }
            """;
            return Task.FromResult(respuesta);
        }
    }
}
