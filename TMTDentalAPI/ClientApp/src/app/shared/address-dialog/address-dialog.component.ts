import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-address-dialog',
  templateUrl: './address-dialog.component.html',
  styleUrls: ['./address-dialog.component.css']
})
export class AddressDialogComponent implements OnInit {
cities = [];
districts = [];
wards = [];
title= "Chọn khu vực";
addresObject = {
  city:null,
  district: null,
  ward: null
};
  constructor() { }

  ngOnInit() {
  }

}
