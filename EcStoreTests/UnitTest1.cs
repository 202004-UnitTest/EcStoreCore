using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EcStoreLib;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private OrderServiceForTest _target;
        private IBookDao _bookDao;

        [SetUp]
        public void Setup()
        {
            _target = new OrderServiceForTest();
            _bookDao = Substitute.For<IBookDao>();
            _target.SetBookDao(_bookDao);
        }

        [Test]
        public void Test_SyncBookOrders_3_Orders_Only_2_book_order()
        {
            // hard to isolate dependency to unit test

            GivenOrders(new List<Order>
            {
                new Order() {Type = "Book"},
                new Order() {Type = "CD"},
                new Order() {Type = "Book"},
            });

            _target.SyncBookOrders();

            ShouldInsertOrder(2, "Book");
            ShouldNotInsertCdOrder("CD");
        }

        private void ShouldNotInsertCdOrder(string type)
        {
            _bookDao.DidNotReceive().Insert(Arg.Is<Order>(order => order.Type == type));
        }

        private void ShouldInsertOrder(int times, string type)
        {
            _bookDao.Received(times).Insert(Arg.Is<Order>(order => order.Type == type));
        }

        private void GivenOrders(List<Order> orders)
        {
            _target.SetOrders(orders);
        }
    }

    internal class OrderServiceForTest : OrderService
    {
        private List<Order> _orders;
        private IBookDao _bookDao;

        internal void SetBookDao(IBookDao bookDao)
        {
            _bookDao = bookDao;
        }

        protected override IBookDao GetBookDao()
        {
            return _bookDao;
        }

        internal void SetOrders(List<Order> orders)
        {
            _orders = orders;
        }

        protected override List<Order> GetOrders()
        {
            return _orders;
        }
    }
}