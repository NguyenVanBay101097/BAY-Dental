import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-service-card-type-apply-dialog',
  templateUrl: './service-card-type-apply-dialog.component.html',
  styleUrls: ['./service-card-type-apply-dialog.component.css']
})
export class ServiceCardTypeApplyDialogComponent implements OnInit {
  price: number = 0;
  computePrice = 'percentage';
  categId: string;
  title: string;
  constructor(public activeModal: NgbActiveModal,) { }

  ngOnInit(): void {
  }

  onApply(){
    this.activeModal.close({price: this.price, computePrice: this.computePrice});
  }

}
