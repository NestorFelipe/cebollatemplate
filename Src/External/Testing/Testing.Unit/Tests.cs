using Moq;
using NUnit.Framework;
using Application.Contracts.Commons;
using Application.Dto.Commons;
using Domain.Entities.BaseModel;
using System.Threading.Tasks;
using Application.Dto.Enums;

namespace Testing.Unit
{
    public class Tests
    {
        private Mock<IBaseGenericCrud> _mockCrud;

        [SetUp]
        public void Setup()
        {
            _mockCrud = new Mock<IBaseGenericCrud>();
        }

        [Test]
        public async Task SaveEntity_DeberiaRetornarRespuestaExitosa()
        {
            // Arrange
            var testDto = new TestDto { Id = 1, Name = "Entidad de Prueba" };
            var expectedResponse = new ResponseAction { Estado = State.Success, Message = "Entidad guardada exitosamente" };

            _mockCrud
                .Setup(crud => crud.SaveEntity<TestEntity, TestDto>(testDto, false))
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _mockCrud.Object.SaveEntity<TestEntity, TestDto>(testDto, false);

            // Assert
            Assert.That(result != null);
            Assert.That(result!.Estado == State.Success);
            
        }


        // Clases de ejemplo para el test
        public class TestDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public class TestEntity : AuditableBaseEntity
        {
            public string Name { get; set; }
        }
    }
}