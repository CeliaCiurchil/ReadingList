using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadingList.Domain.Models
{
    public class Book
    {
        private double _rating;
        private int _yearPublished;

        public int Id { get; private set; }
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public int YearPublished
        {
            get => _yearPublished;
            set
            {
                if (value < 0 || value > DateTime.Now.Year)
                {
                    throw new ArgumentOutOfRangeException("YearPublished must be a valid year.");
                }
                _yearPublished = value;
            }
        }
        public uint Pages { get; set; }
        public string Genre { get; set; } = "";
        public bool Finished { get; set; } = false;
        public double Rating
        {
            get => _rating;
            set
            {
                if (value < 0 || value > 5)
                {
                    throw new ArgumentOutOfRangeException("Rating must be between 0 and 5.");
                }
                _rating = value;
            }
        }

        public Book(int id, string title, string author, int yearPublished, uint pages, string genre, double rating, bool finished = false)
        {
            Id = id;
            Title = title;
            Author = author;
            YearPublished = yearPublished;
            Pages = pages;
            Genre = genre;
            Finished = finished;
            Rating = rating;
        }
    } 
}
