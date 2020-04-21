import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder } from '@angular/forms';
import { PartnerSimple } from 'src/app/partners/partner-simple';
import { CardTypeBasic } from 'src/app/card-types/card-type.service';
import { UserSimple } from 'src/app/users/user-simple';

@Component({
  selector: 'app-service-card-order-create-update',
  templateUrl: './service-card-order-create-update.component.html',
  styleUrls: ['./service-card-order-create-update.component.css']
})
export class ServiceCardOrderCreateUpdateComponent implements OnInit {

  cardOrder: any;
  formGroup: FormGroup;
  filteredPartners: PartnerSimple[];
  filteredInheritedPartner: PartnerSimple[];
  filteredCardTypes: CardTypeBasic[];
  filteredUsers: UserSimple[];

  constructor(private fb: FormBuilder) { }

  ngOnInit() {
    this.cardOrder = {
      state: 'draft'
    };

    this.formGroup = this.fb.group({
      partner: null,
      inheritedPartner: null,
      cardType: null,
      dateOrderObj: null,
      activatedDateObj: null,
      user: null,
      buyType: null
    });
  }

}
