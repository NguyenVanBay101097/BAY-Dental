import { Component, OnInit, ViewChild, Output, EventEmitter, ElementRef } from '@angular/core';
import { AshipRequest, AshipData, PartnerSourceSimple, District, City, Ward } from 'src/app/partners/partner-simple';
import { NgbPopover, NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { FormGroup, FormBuilder } from '@angular/forms';
import { AddressCheckApi } from 'src/app/price-list/price-list';
import { PartnerService } from 'src/app/partners/partner.service';


@Component({
  selector: 'app-check-address-button',
  templateUrl: './check-address-button.component.html',
  styleUrls: ['./check-address-button.component.css']
})
export class CheckAddressButtonComponent implements OnInit {
  @Output() clickAddress : EventEmitter<any> = new EventEmitter<any>();
  @ViewChild('popOver',{static: true}) public popover: NgbPopover;
  addressCheck: AddressCheckApi[] = [];
  isLoading : boolean;

  constructor( 
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
