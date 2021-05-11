import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { NotificationService } from '@progress/kendo-angular-notification';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { ToothDiagnosisService } from '../tooth-diagnosis.service';

@Component({
  selector: 'app-tooth-diagnosis-create-update-dialog',
  templateUrl: './tooth-diagnosis-create-update-dialog.component.html',
  styleUrls: ['./tooth-diagnosis-create-update-dialog.component.css']
})
export class ToothDiagnosisCreateUpdateDialogComponent implements OnInit {

  title: string;
  itemId: string;
  myForm: FormGroup;
  dataSource = [];
  search: string = '';
  loading = false;
  searchUpdate = new Subject<string>();
  submitted = false;
  constructor(
    private fb: FormBuilder, 
    public activeModal: NgbActiveModal,
    private toothDiagnosisService: ToothDiagnosisService,
    private notificationService: NotificationService,
    private productService: ProductService
  ) { }

  ngOnInit() {
    this.myForm = this.fb.group({
      name: [null, Validators.required],
      product: [[], Validators.required]
    });

    setTimeout(() => {
      this.loadServices('');
      if (this.itemId) {
        this.toothDiagnosisService.get(this.itemId).subscribe((result) => {
          this.myForm.patchValue(result);
        }, err => {
          console.log(err);
          this.activeModal.dismiss();
        });
      }
    })
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(value => {
        this.loadServices(value);
      });
  }

  get f() {
    return this.myForm.controls;
  }

  onSave(){
    this.submitted = true;

    if (!this.myForm.valid) {
      return false;
    }

    var value = this.myForm.value;
    value.productIds = value.product.map(x => x.id);
    if (!this.itemId) {
      this.toothDiagnosisService.create(value).subscribe(result => {
        this.submitted = false;
        this.activeModal.close(result);
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }, err => {
        console.log(err);
        this.submitted = false;
      });
    } else {
      this.toothDiagnosisService.update(this.itemId, value).subscribe(result => {
        this.submitted = false;
        this.activeModal.close(result);
        this.notificationService.show({
          content: 'Lưu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
      }, err => {
        console.log(err);
        this.submitted = false;
      });
    }
  }

  loadServices(search?: string){
    this.loading = true;
    var val = new ProductPaged();
    val.search = search;
    val.type2 = 'service';

    this.productService
      .getPaged(val)
      .subscribe(
        (res) => {
          this.dataSource = res.items;
          this.loading = false;
        },
        (err) => {
          console.log(err);
          this.loading = false;
        }
      );
  }

}
