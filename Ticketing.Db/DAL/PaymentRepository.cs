using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ticketing.Db.Models;
using Ticketing.Db.Providers;

namespace Ticketing.Db.DAL
{
    public class PaymentRepository
    {
        private readonly DataAccess _dataAccess;
        private const string TableName = "Payment";

        public PaymentRepository(IConnectionStringProvider connectionStringProvider)
        {
            _dataAccess = new DataAccess(connectionStringProvider.GetConnectionString());
        }

        public IEnumerable<Payment> GetAllPayments()
        {
            const string sql = $"SELECT * FROM {TableName}";
            return _dataAccess.Query<Payment>(sql);
        }

        public Payment GetPaymentById(int paymentId)
        {
            const string sql = $"SELECT * FROM {TableName} WHERE Id = @PaymentId";
            return _dataAccess.QueryFirstOrDefault<Payment>(sql, new { PaymentId = paymentId });
        }

        public void AddPayment(Payment payment)
        {
            const string sql = $"INSERT INTO {TableName} (Status, Amount) VALUES (@Status, @Amount)";
            _dataAccess.Execute(sql, payment);
        }

        public void UpdatePayment(Payment payment)
        {
            const string sql = $"UPDATE {TableName} SET Status = @Status, Amount = @Amount WHERE Id = @Id";
            _dataAccess.Execute(sql, payment);
        }

        public void DeletePayment(int paymentId)
        {
            const string sql = $"DELETE FROM {TableName} WHERE Id = @PaymentId";
            _dataAccess.Execute(sql, new { PaymentId = paymentId });
        }
    }
}
