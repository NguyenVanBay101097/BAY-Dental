import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PartnerDisplay } from '../partner-simple';
import { PartnerService } from '../partner.service';

@Component({
  selector: 'app-partner-supplier-form',
  templateUrl: './partner-supplier-form.component.html',
  styleUrls: ['./partner-supplier-form.component.css']
})
export class PartnerSupplierFormComponent implements OnInit {
  id: string;
  supplier: PartnerDisplay = new PartnerDisplay();
  constructor(
    private activeRoute: ActivatedRoute,
    private partnerService: PartnerService
  ) { }

  ngOnInit() {
    this.id = this.activeRoute.snapshot.paramMap.get('id');
    if (this.id) {
      this.LoadData();
    }
  }

  LoadData() {
    this.partnerService.getPartner(this.id).subscribe((result) => {
      this.supplier = result;
    })
  }

}
