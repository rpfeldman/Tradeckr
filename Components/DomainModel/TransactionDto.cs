using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using System.Text;

namespace DomainModel
{
    public class TransactionDto : IEntity
    {
        private decimal _Value;
        private string _Category = "Uncategorized";

        [Key]
        public int Id { get; set; }
        public decimal Value { get { return _Value; } set { if (value < 1m || value > 1000000000m) { throw new Exception($"property {nameof(Value)} must be on 1 to 1000000000 bound"); } _Value = value; } }
        public DateOnly Date { get; set;  }
        public string Category { get { return _Category; } set { if (string.IsNullOrWhiteSpace(value)) { _Category = "Uncategorized"; } _Category = value; } } 
        public bool Fixed { get; set; }
        public bool Depletion { get; set; }
    }

    public sealed class FixedTransactionDto : TransactionDto
    {
        private int _Duration;
        private int _FixedTransactionId; 
        public FixedTransactionDto()
        {
            Fixed = true;
        }
        public int Duration { get { return _Duration; } set { if (value < 0) { throw new Exception($"property {nameof(Duration)} must be a positive number"); } _Duration = value; }  }

        /// <summary>
        /// /// Works as a parallel identification with the <see cref="TransactionDto.TransactionId"/> from the superclass.. Every fixed transaction has a single identification (<see cref="TransactionDto.TransactionId"/> ) and a identification for its fixed collection
        /// </summary>
        public int FixedTransactionId { get { return _FixedTransactionId; } set { _FixedTransactionId = value; }  }
    }
}
