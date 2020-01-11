import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-shared-error-dialog',
  templateUrl: './shared-error-dialog.component.html',
  styleUrls: ['./shared-error-dialog.component.css']
})
export class SharedErrorDialogComponent implements OnInit {
  title: string;
  body: string;
  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit() {
  }
}

