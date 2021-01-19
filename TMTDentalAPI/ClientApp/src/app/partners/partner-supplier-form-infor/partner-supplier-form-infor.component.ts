import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerSupplierCuDialogComponent } from 'src/app/shared/partner-supplier-cu-dialog/partner-supplier-cu-dialog.component';
import { PartnerDisplay } from '../partner-simple';
import { PartnerService } from '../partner.service';

@Component({
  selector: 'app-partner-supplier-form-infor',
  templateUrl: './partner-supplier-form-infor.component.html',
  styleUrls: ['./partner-supplier-form-infor.component.css']
})
export class PartnerSupplierFormInforComponent implements OnInit {
  id: string;
  supplier: PartnerDisplay = new PartnerDisplay();
  constructor(
    private activeRoute: ActivatedRoute,
    private modalService: NgbModal,
    private partnerService: PartnerService
  ) { }

  ngOnInit() {
    this.id = this.activeRoute.parent.snapshot.paramMap.get('id');
    if (this.id) {
      this.LoadData();
    }
  }
  LoadData() {
    this.partnerService.getPartner(this.id).subscribe((result) => {
      this.supplier = result;
      console.log(result);

    })
  }

  editSupplier() {
    let modalRef = this.modalService.open(PartnerSupplierCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Sửa: ' + this.supplier.name;
    modalRef.componentInstance.id = this.id;

    modalRef.result.then(() => {
      this.LoadData();
    }, () => {
    });
  }
}
