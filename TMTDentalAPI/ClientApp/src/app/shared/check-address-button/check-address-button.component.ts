import { Component, OnInit, ViewChild, Output, EventEmitter, ElementRef } from '@angular/core';
import { AshipRequest, AshipData, PartnerSourceSimple, District, City, Ward } from 'src/app/partners/partner-simple';
import { NgbPopover, NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder } from '@angular/forms';
import { AddressCheckApi } from 'src/app/price-list/price-list';
import { AppSharedShowErrorService } from '../shared-show-error.service';
import { PartnerService } from 'src/app/partners/partner.service';
import { HttpClient } from '@angular/common/http';
import { IntlService } from '@progress/kendo-angular-intl';

@Component({
  selector: 'app-check-address-button',
  templateUrl: './check-address-button.component.html',
  styleUrls: ['./check-address-button.component.css']
})
export class CheckAddressButtonComponent implements OnInit {
  @Output() clickAddress : EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('popOver',{static: true}) public popover: NgbPopover;
  @ViewChild('myinput', {static: true}) myinput: ElementRef;
  id: string;
  formGroup: FormGroup;
  isDisabledDistricts: boolean = true;
  isDisabledWards: boolean = true;
  title: string;
  addressCheck: AddressCheckApi[] = [];
  checkedText: string;
  districtsList: District[] = [];
  provincesList: City[] = [];
  wardsList: Ward[] = [];
  districtsFilter: District[] = [];
  provincesFilter: City[] = [];
  wardsFilter: Ward[] = [];
  cusId: string;
  isLoading : boolean;

  dataSourceCities: Array<{ code: string; name: string }>;
  dataSourceDistricts: Array<{
    code: string;
    name: string;
    cityCode: string;
    cityName: string;
  }>;
  dataSourceWards: Array<{
    code: string;
    name: string;
    districtCode: string;
    districtName: string;
    cityCode: string;
    cityName: string;
  }>;

  dataResultCities: Array<{ code: string; name: string }>;
  dataResultDistricts: Array<{
    code: string;
    name: string;
    cityCode: string;
    cityName: string;
  }>;
  dataResultWards: Array<{
    code: string;
    name: string;
    districtCode: string;
    districtName: string;
    cityCode: string;
    cityName: string;
  }>;

  constructor( private fb: FormBuilder,
    private http: HttpClient,
    private partnerService: PartnerService,
    public activeModal: NgbActiveModal) { }

  ngOnInit() {
    
  }

  checkAddressApi(searchValue: string) {
    var address = searchValue;
    if (!address) {
      return false;
    }
    this.isLoading = true;
    this.partnerService.checkAddressApi(address).subscribe(
      rs => {
        this.isLoading= false;
        this.addressCheck = rs;
        this.addressCheck = this.addressCheck.slice(0, 5);
        console.log(rs);      
      }
    )
  }

  onClickAddress(adr: AddressCheckApi){
    this.clickAddress.emit(adr);
    this.popover.close();
    this.addressCheck = [];
  }

  toggleWithGreeting(popover) {   
    if (popover.isOpen()) {       
      popover.close();
    } else { 
      
      this.addressCheck = [];  
      popover.open();   
    }
  }

   

}
