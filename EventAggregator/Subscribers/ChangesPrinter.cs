using System;
using EventAggregator.Events;

namespace EventAggregator
{
    public class ChangesPrinter : ISubscriber<TableChanged>
    {
        private string _name;

        public ChangesPrinter(string name)
        {
            _name = name;
        }

        public void OnEventHandler(TableChanged e)
        {
            Console.WriteLine($"{_name} {e.Change}");
        }
    }
}