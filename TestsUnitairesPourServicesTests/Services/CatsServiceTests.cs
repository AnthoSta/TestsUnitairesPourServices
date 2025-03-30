using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestsUnitairesPourServices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestsUnitairesPourServices.Data;
using TestsUnitairesPourServices.Models;
using Microsoft.AspNetCore.Cors.Infrastructure;
using NuGet.Protocol;
using TestsUnitairesPourServices.Exceptions;


namespace TestsUnitairesPourServices.Services.Tests
{
    [TestClass()]
    public class CatsServiceTests
    {
        private DbContextOptions<ApplicationDBContext> _options;
        public CatsServiceTests() 
        {
            _options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(databaseName: "CatsService")
                .UseLazyLoadingProxies(true)
                .Options;
        }

        [TestInitialize]
        public void Init()
        {
            using ApplicationDBContext db = new ApplicationDBContext(_options);
            House[] houses = new House[]
            {
                new House
                {
                    Id = 1,
                    Address = "500 rue Blainville",
                    OwnerName = "Jean-Paul Vincent"
                },new House
                {
                    Id = 2,
                    Address = "666 rue de l'Enfer",
                    OwnerName = "Bob"
                }
            };
            Cat[] cats = new Cat[]
            {
                new Cat
                {
                    Id = 1,
                    Name = "Garfield",
                    House = houses[0],
                    Age = 5
                }, new Cat
                {
                    Id = 2,
                    Name = "Petit Chat",
                    Age = 10
                    
                }
            };
            db.House.AddRange(houses);
            db.Cat.AddRange(cats);
            db.SaveChanges();
        }

        [TestCleanup]
        public void Dispose()
        {
            using ApplicationDBContext db = new ApplicationDBContext(_options);
            db.Cat.RemoveRange(db.Cat);
            db.House.RemoveRange(db.House);
            db.SaveChanges();
        }

        [TestMethod()]
        public void MoveTest()
        {
            using ApplicationDBContext db = new ApplicationDBContext(_options);
            CatsService service = new CatsService(db);
            House maison1 = db.House.Find(1);
            House maison2 = db.House.Find(2);
            Assert.IsNotNull(service.Move(1, maison1, maison2));
        }

        [TestMethod()]
        public void MoveTestChatNull()
        {
            using ApplicationDBContext db = new ApplicationDBContext(_options);
            CatsService service = new CatsService(db);
            House maison1 = db.House.Find(1);
            House maison2 = db.House.Find(2);
            Assert.IsNull(service.Move(3, maison1, maison2));
        }

        [TestMethod()]
        public void MoveTestChatSansMaison()
        {
            using ApplicationDBContext db = new ApplicationDBContext(_options);
            CatsService service = new CatsService(db);
            House maison1 = db.House.Find(1);
            House maison2 = db.House.Find(2);
            Assert.AreEqual(Assert.ThrowsException<WildCatException>(() => service.Move(2, maison1, maison2)).Message, "On n'apprivoise pas les chats sauvages");
            
        }

        [TestMethod()]
        public void MoveTestMauvaiseMaisonOrigine()
        {
            using ApplicationDBContext db = new ApplicationDBContext(_options);
            CatsService service = new CatsService(db);
            House maison1 = db.House.Find(1);
            House maison2 = db.House.Find(2);
            Assert.AreEqual(Assert.ThrowsException<DontStealMyCatException>(() => service.Move(1,  maison2, maison1)).Message, "Touche pas à mon chat!");
        }
    }
}