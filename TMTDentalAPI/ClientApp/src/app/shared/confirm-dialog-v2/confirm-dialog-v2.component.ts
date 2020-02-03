import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-confirm-dialog-v2',
  templateUrl: './confirm-dialog-v2.component.html',
  styleUrls: ['./confirm-dialog-v2.component.css']
})
export class ConfirmDialogV2Component implements OnInit {
  title: string;
  body: string;
  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit() {
  }

  onConfirm() {
    this.activeModal.close(true);
  }

  onCancel() {
    this.activeModal.close(false);
  }
}
