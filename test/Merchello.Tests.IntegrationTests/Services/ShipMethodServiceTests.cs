﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.Data;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class ShipMethodServiceTests : BaseUsingSqlServerSyntax
    {
        private IShipMethodService _shipMethodService;

        [SetUp]
        public void Setup()
        {
            _shipMethodService = new ShipMethodService();
        }

        [Test]
        public void Can_Create_A_ShipMethod()
        {
            IShipMethod created = null;

            ShipMethodService.Created += delegate(IShipMethodService sender, NewEventArgs<IShipMethod> args)
                {
                    created = args.Entity;
                };

            var shipmethod = _shipMethodService.CreateShipMethod("Truck and Stuff", Guid.NewGuid(), ShipMethodType.Carrier);

            Assert.NotNull(created);
            Assert.AreEqual("Truck and Stuff", shipmethod.Name);
        }

        [Test]
        public void Can_Save_A_ShipMethod()
        {
            var shipmethod = ShipmentData.MockShipMethodForInserting();
            Assert.IsFalse(shipmethod.HasIdentity);
            _shipMethodService.Save(shipmethod);

            Assert.IsTrue(shipmethod.HasIdentity);

        }

        [Test]
        public void Can_Delete_A_ShipMethod_With_AssociatedShipments()
        {
            var shipmentService = new ShipmentService();
            var shipments = shipmentService.GetAll();

            var shipment = shipments.FirstOrDefault(x => x.ShipMethodId != null);
            var id = shipment.Id;
            Console.WriteLine(string.Format("shipment: {0} - ship method: {1}", shipment.Id, shipment.ShipMethodId));
            if (shipment != null && shipment.ShipMethodId != null)
            {
                var shipmethod = _shipMethodService.GetById(shipment.ShipMethodId.Value);
                _shipMethodService.Delete(shipmethod);

                shipment = shipmentService.GetById(id);
                Assert.IsNull(shipment.ShipMethodId);
            }
            else
            {
                Assert.Pass();
            }
        }
    }
}