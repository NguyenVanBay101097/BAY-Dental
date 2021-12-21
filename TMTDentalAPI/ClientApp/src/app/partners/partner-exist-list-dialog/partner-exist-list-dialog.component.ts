import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-partner-exist-list-dialog',
  templateUrl: './partner-exist-list-dialog.component.html',
  styleUrls: ['./partner-exist-list-dialog.component.css']
})
export class PartnerExistListDialogComponent implements OnInit {

  title = "Trùng khách hàng";
  data = [];
  constructor(
    public activeModal: NgbActiveModal
  ) { }

  ngOnInit(): void {
  }

  onClickName(){
    this.activeModal.dismiss();
  }
}
