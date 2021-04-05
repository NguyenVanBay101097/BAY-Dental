import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { result } from 'lodash';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { ToothDiagnosisPaged, ToothDiagnosisService } from 'src/app/tooth-diagnosis/tooth-diagnosis.service';

@Component({
  selector: 'app-tooth-diagnosis-popover',
  templateUrl: './tooth-diagnosis-popover.component.html',
  styleUrls: ['./tooth-diagnosis-popover.component.css']
})
export class ToothDiagnosisPopoverComponent implements OnInit {

  allType = {
    diagnosis : 'diagnosis',
    service: 'service'
  }

  @Input() type = this.allType.diagnosis; // 'diagnosis': chẩn đoán ,'service': dịch vụ
  @Input() tags = [];
  dataSource = [];
  @Input() title = '';
  searchUpdatePopOver = new Subject<string>();
  @Input() popOverPlace = 'right';
  @Output() onSave = new EventEmitter();
  @ViewChild('popOver', { static: true }) public popover: any;
  @ViewChild('multiSelect', { static: true }) multiSelect: MultiSelectComponent;
  constructor(
    private toothDiagnosisService: ToothDiagnosisService,
    private productService: ProductService
  ) { }

  ngOnInit() {
    setTimeout(() => {
      this.searchUpdatePopOver.pipe(
        debounceTime(400),
        distinctUntilChanged())
        .subscribe(value => {
          this.loadPopOver(value);
        });
    }, 200);
  }

  toggleWithTags(popover, mytags) {
    if (popover.isOpen()) {
      popover.close();
    } else {
      this.loadPopOver();
      popover.open({ mytags });
    }
  }

  getPageDiagnosis(){
    var val = new ToothDiagnosisPaged();
    val.limit = 1000;
    val.offset = 0;
    val.search = '';
    this.toothDiagnosisService.getPaged(val).subscribe(result => {
      this.dataSource = result.items;
    })
  }

  getPageProduct(){
    var val = new ProductPaged();
    val.limit = 0;
    val.offset = 0;
    val.search = '';
    val.type2 = 'service';
    this.productService.getPaged(val).subscribe(result => {
      this.dataSource = result.items;
    })
  }

  loadPopOver(q?: string) {
    if(this.type == this.allType.diagnosis) {
      this.getPageDiagnosis();
    }
    if(this.type == this.allType.service){
      this.getPageProduct();
    }
  }
  update(tags) {
    this.popover.close();
    this.onSave.emit(tags);
  }
}
