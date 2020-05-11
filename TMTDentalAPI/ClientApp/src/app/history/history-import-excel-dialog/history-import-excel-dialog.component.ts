import { HistoryService, HistoryImportExcelBaseViewModel } from './../history.service';
import { Component, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
@Component({
  selector: 'app-history-import-excel-dialog',
  templateUrl: './history-import-excel-dialog.component.html',
  styleUrls: ['./history-import-excel-dialog.component.css']
})

export class HistoryImportExcelDialogComponent implements OnInit {
  fileBase64 = '';
  type: string;
  errors: string[];
  title: string;
 

  constructor( private historyService: HistoryService,public activeModal: NgbActiveModal) { }

  ngOnInit() {
  }
  onFileChange(value) {
    this.fileBase64 = value;
  }

  onSave() {
    debugger
    if (!this.fileBase64 || this.fileBase64 === '') {
      return;
    }
    var val = new HistoryImportExcelBaseViewModel();
    val.fileBase64 = this.fileBase64;
    this.actionImport(val).subscribe((result: any) => {
      if (result.success) {
        this.activeModal.close(true);
      }else{
        this.errors = result.errors;
      }
    });
  }

  actionImport(val: any) {   
      return this.historyService.importExcel(val);   
  }

  onCancel() {
    this.activeModal.dismiss();
  }
}
