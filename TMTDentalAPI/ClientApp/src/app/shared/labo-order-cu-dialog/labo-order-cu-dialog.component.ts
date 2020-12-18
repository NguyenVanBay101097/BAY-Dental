import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-labo-order-cu-dialog',
  templateUrl: './labo-order-cu-dialog.component.html',
  styleUrls: ['./labo-order-cu-dialog.component.css']
})
export class LaboOrderCuDialogComponent implements OnInit {
  title: string;
  myForm: FormGroup;

  NCCLaboList: any = [];

  constructor(private fb: FormBuilder, public activeModal: NgbActiveModal) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      NCCLabo: [null, Validators.required], 
      dateObj: null, 
    });

    setTimeout(() => {
      this.loadNCCLaboList();

    });
  }

  loadNCCLaboList() {

  }

  onSave() {

  }
}
