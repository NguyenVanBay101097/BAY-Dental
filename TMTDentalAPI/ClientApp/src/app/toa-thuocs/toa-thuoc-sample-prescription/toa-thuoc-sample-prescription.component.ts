import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { SamplePrescriptionsPaged, SamplePrescriptionsService, SamplePrescriptionsSimple } from 'src/app/sample-prescriptions/sample-prescriptions.service';
import { AppSharedShowErrorService } from 'src/app/shared/shared-show-error.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmDialogComponent } from 'src/app/shared/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-toa-thuoc-sample-prescription',
  templateUrl: './toa-thuoc-sample-prescription.component.html',
  styleUrls: ['./toa-thuoc-sample-prescription.component.css']
})
export class ToaThuocSamplePrescriptionComponent implements OnInit {

  search: string;
  searchUpdate = new Subject<string>();
  name: string;
  samplePrescriptions: SamplePrescriptionsSimple[] = [];
  @Output() nameSamplePrescription = new EventEmitter<string>();
  @Output() itemSamplePrescription = new EventEmitter<string>();

  constructor(private samplePrescriptionsService: SamplePrescriptionsService, 
    private errorService: AppSharedShowErrorService, private modalService: NgbModal) { }

  ngOnInit() {
    setTimeout(() => {
      this.loadSamplePrescriptionsList();
    });

    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadSamplePrescriptionsList(); 
      });
  }

  loadSamplePrescriptionsList() {
    var val = new SamplePrescriptionsPaged();
    val.offset = 0;
    val.limit = 20;
    val.search = this.search || '';
    this.samplePrescriptionsService.getPaged(val).subscribe(result => {
      this.samplePrescriptions = result.items;
    });
  }

  deleteItem(item) {
    let modalRef = this.modalService.open(ConfirmDialogComponent, { size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Xóa: ' + item.name;
    modalRef.result.then(() => {
      this.samplePrescriptionsService.delete(item.id).subscribe(() => {
        this.loadSamplePrescriptionsList();
      }, err => {
        this.errorService.show(err);
      });
    }, () => {
    });
  }

  saveItem() {
    this.nameSamplePrescription.emit(this.name);
    this.loadSamplePrescriptionsList();
  }

  selectItem(item) {
    this.itemSamplePrescription.emit(item);
  }
}
