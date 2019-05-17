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
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test_SyncBookOrders_3_Orders_Only_2_book_order()
        {
            // hard to isolate dependency to unit test

            var target = new OrderServiceForTest();
            target.SetOrders(new List<Order>
            {
                new Order() {Type = "Book"},
                new Order() {Type = "CD"},
                new Order() {Type = "Book"},
            });

            var bookDao = Substitute.For<IBookDao>();
            target.SetBookDao(bookDao);
            target.SyncBookOrders();

            bookDao.Received(2).Insert(Arg.Is<Order>(order => order.Type == "Book"));
            bookDao.DidNotReceive().Insert(Arg.Is<Order>(order => order.Type == "CD"));
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