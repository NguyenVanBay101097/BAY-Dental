import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { NgbActiveModal, NgbPopover } from '@ng-bootstrap/ng-bootstrap';
import { PartnerService } from 'src/app/partners/partner.service';
import { AddressCheckApi } from 'src/app/price-list/price-list';


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
