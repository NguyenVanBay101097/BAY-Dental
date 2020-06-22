import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { ToaThuocLineSave } from '../toa-thuoc.service';

@Component({
  selector: 'app-toa-thuoc-lines-save-cu-form',
  templateUrl: './toa-thuoc-lines-save-cu-form.component.html',
  styleUrls: ['./toa-thuoc-lines-save-cu-form.component.css']
})
export class ToaThuocLinesSaveCuFormComponent implements OnInit {
  showCreateOrEdit: boolean = false;
  lines: any[] = [];
  indexLineEdit: number;
  @Input() dataThuocsReceive: any[];
  @Output() dataThuocsSend = new EventEmitter<any[]>();

  constructor() { }

  ngOnInit() {
    console.log("Pro", this.dataThuocsReceive);
    this.lines = this.dataThuocsReceive;
  }

  onCreate() {
    this.showCreateOrEdit = true;
  }

  getShowCreateOrEdit(value) {
    this.showCreateOrEdit = value;
  }

  getDataThuocSend(value) {
    console.log(value);
    if (this.indexLineEdit || this.indexLineEdit == 0) {
      this.lines[this.indexLineEdit] = value;
    } else {
      this.lines.push(value);
    }
  }
}
