﻿using Hotel.Data.Dtos;
using Hotel.Data.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.ViewModels.RoomTypes
{
    public class RoomTypeVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Content { get; set; }
        public string? Slug { get; set; }
        public int? Capacity { set; get; }
        public decimal? Price { set; get; }
        public string? View { get; set; }
        public string? BedType { get; set; }
        public string? Size { get; set; }
        public string? Status { set; get; }
        public List<RoomImageModel>? RoomImages { set; get; }
        public List<RoomFacilityModel>? RoomFacilitys { set; get; }

    }
}
