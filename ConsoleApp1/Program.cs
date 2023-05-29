using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;



// Define the Customer model
public class Customer
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    [StringLength(2)]
    public string Country { get; set; }

    public bool IsCompany { get; set; }

    [StringLength(50)]
    public string TaxIdentificationNumber { get; set; }
}

// Define the Elements model
public class Elements
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(15,2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(15,2)")]
    public decimal VatRate { get; set; }
}

// Define the SalesInvoice model
public class SalesInvoice
{
    [Key]
    public int Id { get; set; }

    public DateTime CreatedDate { get; set; }

    [StringLength(20)]
    public string DocumentNumber { get; set; }

    [ForeignKey("CustomerId")]
    public Customer Customer { get; set; }

    public List<Elements> Elements { get; set; }
}

// Define the DbContext
public class DatabaseContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Elements> Elements { get; set; }
    public DbSet<SalesInvoice> SalesInvoices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=WBA_DatabaseConcepts_1;Trusted_Connection=SSPI;Encrypt=false;TrustServerCertificate=true;Integrated Security=True;");
    }
}

// Create repositories
public class SalesInvoiceRepository
{
    private readonly DatabaseContext _context;

    public SalesInvoiceRepository(DatabaseContext context)
    {
        _context = context;
    }

    public void Save(SalesInvoice salesInvoice)
    {
        _context.SalesInvoices.Add(salesInvoice);
        _context.SaveChanges();
    }

    public List<SalesInvoice> GetAll()
    {
        return _context.SalesInvoices.ToList();
    }
}

public class CustomerRepository
{
    private readonly DatabaseContext _context;

    public CustomerRepository(DatabaseContext context)
    {
        _context = context;
    }

    public void Save(Customer customer)
    {
        _context.Customers.Add(customer);
        _context.SaveChanges();
    }

    public List<Customer> GetAll()
    {
        return _context.Customers.ToList();
    }
}

// Example usage
class Program
{
    static void Main(string[] args)
    {
        using (var context = new DatabaseContext())
        {
            var customerRepository = new CustomerRepository(context);
            var salesInvoiceRepository = new SalesInvoiceRepository(context);

            // Create instances of models
            var customer1 = new Customer
            {
                Name = "John Doe",
                Country = "US",
                IsCompany = false,
                TaxIdentificationNumber = "12345"
            };

            var elements1 = new Elements
            {
                Name = "Product A",
                Quantity = 10,
                Price = 9.99m,
                VatRate = 0.21m
            };

            var salesInvoice = new SalesInvoice
                {
                    CreatedDate = DateTime.Now,
                    DocumentNumber = "INV-001",
                    Customer = customer1,
                    Elements = new List<Elements> { elements1 }
                };

            // Save the customer and sales invoice
            customerRepository.Save(customer1);
            salesInvoiceRepository.Save(salesInvoice);

            // Retrieve all sales invoices
            var allSalesInvoices = salesInvoiceRepository.GetAll();
            foreach (var invoice in allSalesInvoices)
            {
                Console.WriteLine($"Invoice ID: {invoice.Id}, Created Date: {invoice.CreatedDate}, " +
                                  $"Document Number: {invoice.DocumentNumber}, Customer: {invoice.Customer.Name}");
            }
        }
    }
}