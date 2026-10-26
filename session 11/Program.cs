using System;
using System.Collections.Generic;

namespace Session08_Assignment
{
    // ==========================================
    // SECTION 01: Book Library System
    // ==========================================

    public class Book
    {
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string[] Authors { get; set; }
        public DateTime PublicationDate { get; set; }
        public decimal Price { get; set; }

        public Book(string _ISBN, string _Title, string[] _Authors, DateTime _PublicationDate, decimal _Price)
        {
            ISBN = _ISBN;
            Title = _Title;
            Authors = _Authors;
            PublicationDate = _PublicationDate;
            Price = _Price;
        }

        public override string ToString()
        {
            string authorsList = Authors != null ? string.Join(", ", Authors) : "No Authors";
            return $"ISBN: {ISBN}, Title: '{Title}', Authors: [{authorsList}], Date: {PublicationDate.ToShortDateString()}, Price: {Price:C}";
        }
    }

    public class BookFunctions
    {
        public static string GetTitle(Book B)
        {
            return B?.Title ?? "N/A";
        }

        public static string GetAuthors(Book B)
        {
            if (B?.Authors == null || B.Authors.Length == 0)
                return "No Authors";
            return string.Join(", ", B.Authors);
        }

        public static string GetPrice(Book B)
        {
            return B != null ? B.Price.ToString("C") : "N/A";
        }
    }

    // User-defined delegate for Section 01
    public delegate string BookDelegate(Book B);

    public class LibraryEngine
    {
        // Parameterized ProcessBooks using User-defined delegate
        public static void ProcessBooks(List<Book> bList, BookDelegate fPtr)
        {
            foreach (Book B in bList)
            {
                Console.WriteLine(fPtr(B));
            }
        }

        // Overloaded ProcessBooks using Built-in Func<> delegate
        public static void ProcessBooks(List<Book> bList, Func<Book, string> fPtr)
        {
            foreach (Book B in bList)
            {
                Console.WriteLine(fPtr(B));
            }
        }
    }


    // ==========================================
    // SECTION 02: Order Processing System
    // ==========================================

    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public Order(int id, string customerName, decimal price, int quantity)
        {
            Id = id;
            CustomerName = customerName;
            Price = price;
            Quantity = quantity;
        }
    }

    // Part 1 — User-Defined Delegate
    public delegate decimal PriceCalculator(Order order);

    public class OrderService
    {
        // Event for Order processing completed (Part 5)
        public event Action<Order> OrderProcessed;

        // Part 1: Calculate Order Price using user-defined delegate
        public decimal CalculateOrderPrice(Order order, PriceCalculator calculator)
        {
            return calculator(order);
        }

        // Part 2: Calculate Order Price using Func<> delegate (Bonus Strategies Support)
        public decimal CalculateOrderPrice(Order order, Func<Order, decimal> calculator)
        {
            return calculator(order);
        }

        // Part 3: Validate Order using Predicate<>
        public bool ValidateOrder(Order order, Predicate<Order> validationRule)
        {
            return validationRule(order);
        }

        // Part 4: Execute actions after processing
        public void ProcessOrder(Order order, Action<Order> action)
        {
            action(order);
        }

        // Part 5: Process Order and Raise Event
        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"[OrderService] Processing Order ID: {order.Id}...");

            // Raise Event if subscribed
            OrderProcessed?.Invoke(order);
        }
    }


    // ==========================================
    // MAIN PROGRAM
    // ==========================================

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine("             SECTION 01: BOOK SYSTEM              ");
            Console.WriteLine("==================================================");

            List<Book> books = new List<Book>
            {
                new Book("123-456", "C# Programming", new[] { "John Doe", "Jane Smith" }, new DateTime(2023, 5, 10), 49.99m),
                new Book("987-654", "Design Patterns", new[] { "Erich Gamma" }, new DateTime(2021, 1, 15), 59.99m)
            };

            // 1. User-Defined Delegate
            Console.WriteLine("\n--- 1. User-Defined Delegate (GetTitle) ---");
            BookDelegate titleDelegate = BookFunctions.GetTitle;
            LibraryEngine.ProcessBooks(books, titleDelegate);

            // 2. Built-In Func<> Delegate
            Console.WriteLine("\n--- 2. Built-In Func<> Delegate (GetAuthors) ---");
            Func<Book, string> authorsFunc = BookFunctions.GetAuthors;
            LibraryEngine.ProcessBooks(books, authorsFunc);

            // 3. Anonymous Method (GetISBN)
            Console.WriteLine("\n--- 3. Anonymous Method (GetISBN) ---");
            Func<Book, string> isbnAnonymous = delegate (Book B)
            {
                return B?.ISBN ?? "No ISBN";
            };
            LibraryEngine.ProcessBooks(books, isbnAnonymous);

            // 4. Lambda Expression (GetPublicationDate)
            Console.WriteLine("\n--- 4. Lambda Expression (GetPublicationDate) ---");
            Func<Book, string> pubDateLambda = B => B.PublicationDate.ToShortDateString();
            LibraryEngine.ProcessBooks(books, pubDateLambda);


            Console.WriteLine("\n==================================================");
            Console.WriteLine("         SECTION 02: ORDER PROCESSING SYSTEM       ");
            Console.WriteLine("==================================================");

            Order sampleOrder = new Order(101, "Abanoub", 100.00m, 3);
            OrderService orderService = new OrderService();

            // Part 1 — User-Defined Delegate
            Console.WriteLine("\n--- Part 1: User-Defined Delegate ---");
            PriceCalculator normalCalc = o => o.Price * o.Quantity;
            PriceCalculator discountCalc = o => (o.Price * o.Quantity) - 20m;
            Console.WriteLine($"Total Price (Normal): {orderService.CalculateOrderPrice(sampleOrder, normalCalc):C}");
            Console.WriteLine($"Total Price (Discounted): {orderService.CalculateOrderPrice(sampleOrder, discountCalc):C}");

            // Part 2 & Bonus Challenge — Func<> & Pricing Strategies
            Console.WriteLine("\n--- Part 2 & Bonus: Pricing Strategies with Func<> ---");
            Func<Order, decimal> normalStrategy = o => o.Price * o.Quantity;
            Func<Order, decimal> tenPercentDiscount = o => (o.Price * o.Quantity) * 0.90m;
            Func<Order, decimal> twentyPercentDiscount = o => (o.Price * o.Quantity) * 0.80m;
            Func<Order, decimal> vipDiscount = o => (o.Price * o.Quantity) * 0.50m; // 50% for VIP

            Console.WriteLine($"Normal Price: {orderService.CalculateOrderPrice(sampleOrder, normalStrategy):C}");
            Console.WriteLine($"10% Discount: {orderService.CalculateOrderPrice(sampleOrder, tenPercentDiscount):C}");
            Console.WriteLine($"20% Discount: {orderService.CalculateOrderPrice(sampleOrder, twentyPercentDiscount):C}");
            Console.WriteLine($"VIP Discount: {orderService.CalculateOrderPrice(sampleOrder, vipDiscount):C}");

            // Part 3 — Predicate<> Validation Rules
            Console.WriteLine("\n--- Part 3: Order Validation (Predicate<>) ---");
            Predicate<Order> checkQuantity = o => o.Quantity > 0;
            Predicate<Order> checkPrice = o => o.Price > 0;
            Predicate<Order> checkCustomer = o => !string.IsNullOrEmpty(o.CustomerName);

            bool isValid = orderService.ValidateOrder(sampleOrder, checkQuantity) &&
                           orderService.ValidateOrder(sampleOrder, checkPrice) &&
                           orderService.ValidateOrder(sampleOrder, checkCustomer);

            Console.WriteLine($"Is Order Valid? {isValid}");

            // Part 4 — Action<> Post-Processing Actions
            Console.WriteLine("\n--- Part 4: Action<> Execution ---");
            Action<Order> printOrder = o => Console.WriteLine($"[Action 1] Order #{o.Id} processed for {o.CustomerName}");
            Action<Order> sendConfirmation = o => Console.WriteLine($"[Action 2] Confirmation email sent to {o.CustomerName}");
            Action<Order> auditLog = o => Console.WriteLine($"[Action 3] Audit log recorded for Order #{o.Id}");

            orderService.ProcessOrder(sampleOrder, printOrder);
            orderService.ProcessOrder(sampleOrder, sendConfirmation);
            orderService.ProcessOrder(sampleOrder, auditLog);

            // Part 5 & 6 — Events, Multicast Delegates, Subscription & Unsubscription
            Console.WriteLine("\n--- Part 5 & 6: Events & Multicast Behavior ---");

            // Define Event Handlers
            Action<Order> handler1 = o => Console.WriteLine($"[Event Handler 1] UI Updated for Order #{o.Id}");
            Action<Order> handler2 = o => Console.WriteLine($"[Event Handler 2] SMS Notification Sent for Order #{o.Id}");
            Action<Order> handler3 = o => Console.WriteLine($"[Event Handler 3] Inventory Database Updated");

            // Subscribe Handlers
            orderService.OrderProcessed += handler1;
            orderService.OrderProcessed += handler2;
            orderService.OrderProcessed += handler3;

            Console.WriteLine("\nTriggering Event (All 3 Handlers Subscribed):");
            orderService.ProcessOrder(sampleOrder);

            // Unsubscribe Handler 1
            Console.WriteLine("\nUnsubscribing Handler 1...");
            orderService.OrderProcessed -= handler1;

            Console.WriteLine("\nTriggering Event Again (Handler 1 Unsubscribed):");
            orderService.ProcessOrder(sampleOrder);


            // Part 7 — Complete End-to-End Flow
            Console.WriteLine("\n==================================================");
            Console.WriteLine("          Part 7: Complete End-to-End Flow        ");
            Console.WriteLine("==================================================");

            Order finalOrder = new Order(202, "Sarah", 150.00m, 2);

            // Step 1: Validate
            if (orderService.ValidateOrder(finalOrder, o => o.Quantity > 0 && o.Price > 0))
            {
                // Step 2: Calculate Price
                decimal finalPrice = orderService.CalculateOrderPrice(finalOrder, vipDiscount);
                Console.WriteLine($"Order Validated! Total Price (VIP Rate): {finalPrice:C}");

                // Step 3 & 4: Process Order and Raise Event
                orderService.ProcessOrder(finalOrder);
            }
        }
    }

    /*
    ===================================================================================
    QUESTIONS & ANSWERS (THEORY SECTION)
    ===================================================================================

    Q1: What is the difference between PriceCalculator and Func<Order, decimal>?
    -----------------------------------------------------------------------------------
    - PriceCalculator is a custom (user-defined) delegate explicit to this namespace.
    - Func<Order, decimal> is a generic built-in delegate provided by .NET framework.
    - Functionally both do the same thing, but Func reduces boilerplate code by eliminating
      the need to declare explicit delegate types.

    Q2: What is the difference between Action<Order> and Func<Order, decimal>?
    -----------------------------------------------------------------------------------
    - Action<Order> represents a method that takes an Order parameter and returns void (no return value).
    - Func<Order, decimal> represents a method that takes an Order parameter and returns a value of type decimal.

    Q3: Why does Predicate<T> return bool? What kind of problem is it designed to represent?
    -----------------------------------------------------------------------------------
    - Predicate<T> is specifically designed to represent conditions, criteria, or logical assertions.
    - It always returns a boolean (true/false) to evaluate whether a given object satisfies a specific condition (e.g., validation rules, filtering lists).

    Q4: What is the difference between a delegate and an event?
    -----------------------------------------------------------------------------------
    - A Delegate is a type-safe function pointer. Any code with reference to a delegate can invoke it, reassign it, or overwrite its invocation list.
    - An Event is an encapsulation/wrapper around a delegate based on the Publisher-Subscriber pattern.
      External code can only subscribe (+=) or unsubscribe (-=) from an event; only the declaring class can invoke/trigger or clear it.

    Q5: Why can't external code normally invoke an event declared in another class?
    -----------------------------------------------------------------------------------
    - For security and encapsulation reasons. Events are designed so that only the publisher class controls when the event happens.
      Allowing external callers to invoke events would break event encapsulation and cause unauthorized notifications.

    Q6: What happens when multiple handlers subscribe to the same event?
    -----------------------------------------------------------------------------------
    - The event internal delegate creates a Multicast Delegate chain. When raised, it calls all registered handlers sequentially in the order they were subscribed.

    Q7: Explain what this code means: orderService.OrderProcessed += HandleOrderProcessed;
    -----------------------------------------------------------------------------------
    - OrderProcessed: The event defined in the OrderService publisher class.
    - +=: The addition assignment operator used to subscribe/add a new listener to the event multicast chain.
    - HandleOrderProcessed: The method/handler being attached, which matches the delegate signature (Action<Order>).

    Q8 — Challenge: Explain the difference between Action<Order> and event Action<Order>?
    -----------------------------------------------------------------------------------
    - Exposing public Action<Order> field/property allows external code to directly call it (Action.Invoke())
      or overwrite all existing subscribers by assigning it directly (Action = myMethod).
    - Exposing event Action<Order> restricts external code to only using += and -=, ensuring that no external caller can reset the subscribers list or trigger the event prematurely.
    */
}