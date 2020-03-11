import { Component, OnInit, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-facebook-dialog',
  templateUrl: './facebook-dialog.component.html',
  styleUrls: ['./facebook-dialog.component.css']
})
export class FacebookDialogComponent implements OnInit {

  @Input() DataUser: any[];
  @Input() DataFanpages: any[];

  selectedPage: any = null;

  constructor(public activeModal: NgbActiveModal) { }

  ngOnInit() {
  }

  onSave() {
    this.activeModal.close(this.selectedPage);
  }

  selectPage(item) {
    if (this.selectedPage === item) {
      this.selectedPage = null;
    } else {
      this.selectedPage = item;
    }
  }
}
