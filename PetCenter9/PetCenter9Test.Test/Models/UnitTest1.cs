using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetCenter9.Controllers;
using PetCenter9.Data;
using PetCenter9.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace PetCenter9Test.Test.Models
{

    [TestClass]
    public class UnitTest1
    {
        private DbContextOptions<PetCenter9Context> _dbContextOptions;

        [TestInitialize]
        public void Initialize()
        {
            _dbContextOptions = new DbContextOptionsBuilder<PetCenter9Context>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        [TestMethod]
        public async Task Create_ValidOwner_ReturnsRedirectToActionResult()
        {
            using (var dbContext = new PetCenter9Context(_dbContextOptions))
            {
                var controller = new VaccinesController(dbContext);
                var vaccine = new Vaccines
                {
                    Name = "Jovana",

                };

                var result = await controller.Create(vaccine) as RedirectToActionResult;

                Assert.IsNotNull(result);
                Assert.AreEqual("Index", result.ActionName);
            }
        }

        [TestMethod]
        public async Task Details_NonExistingOwnerId()
        {
            using (var dbContext = new PetCenter9Context(_dbContextOptions))
            {
                var controller = new VaccinesController(dbContext);

                var result = await controller.Details(100) as NotFoundResult;

                Assert.IsNotNull(result);
            }
        }

    }
}