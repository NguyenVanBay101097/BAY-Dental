import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { IntlService } from '@progress/kendo-angular-intl';
import { CashBookService } from '../cash-book.service';

@Component({
  selector: 'app-cash-book-cu-dialog',
  templateUrl: './cash-book-cu-dialog.component.html',
  styleUrls: ['./cash-book-cu-dialog.component.css']
})
export class CashBookCuDialogComponent implements OnInit {

  type: string = null;
  item: any = null;
  title: string = null;
  formGroup: FormGroup;
  submitted: boolean = false;

  get f() { return this.formGroup.controls; }
  
  constructor(
    private fb: FormBuilder,
    public activeModal: NgbActiveModal,
    private intelservice: IntlService,
    private cashBookService: CashBookService
  ) { }

  ngOnInit() {
    this.getTitle();
    this.formGroup = this.fb.group({

    });
  }

  getType() {
    if (this.type == "thu") {
      return "phiếu thu";
    } else {
      return "phiếu chi";
    }
  }

  getTitle() {
    if (this.item) {
      this.title = "Chỉnh sửa " + this.getType();
    } else {
      this.title = "Tạo " + this.getType();
    }
  }
}
