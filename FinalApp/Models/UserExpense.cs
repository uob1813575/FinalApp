﻿
namespace FinalApp.Models {
    using System;
    using Newtonsoft.Json;
    using Xamarin.Forms;
    using J = Newtonsoft.Json.JsonPropertyAttribute;
    using N = Newtonsoft.Json.NullValueHandling;

    public partial class UserExpense {
        public string Id { get; set; }
        [J("amount")] public double Amount { get; set; }
        [J("description")] public string Description { get; set; }
        [J("startDate")] public DateTimeOffset StartDate { get; set; }
        [J("expireDate")] public DateTimeOffset ExpireDate { get; set; }
        [J("categoryId")] public string CategoryId { get; set; }
    }

    public partial class UserExpense {
        [JsonIgnore]
        public Category UserCategory { get; set; }
    }

    public partial class UserExpense {
        public static UserExpense FromJson(string json) => JsonConvert.DeserializeObject<UserExpense>(json, Commons.StandardJsonConverter.Settings);
    }

    public partial class UserExpense { 
        [JsonIgnore]
        public string CategoryName { 
            get {
                switch (CategoryId) {
                    case "1": return "House";
                    case "2": return "Car";
                    case "3": return "Entertainment";
                    default: return "Other";
                }
            }
        }

    }


}

