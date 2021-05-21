import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { anyChanged } from '@progress/kendo-angular-common';
import { MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { result } from 'lodash';
import { Observable, of, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, map, switchMap } from 'rxjs/operators';
import { ProductPaged, ProductService } from 'src/app/products/product.service';
import { ToothDiagnosisPaged, ToothDiagnosisSave, ToothDiagnosisService } from 'src/app/tooth-diagnosis/tooth-diagnosis.service';

@Component({
  selector: 'app-tooth-diagnosis-popover',
  templateUrl: './tooth-diagnosis-popover.component.html',
  styleUrls: ['./tooth-diagnosis-popover.component.css']
})
export class ToothDiagnosisPopoverComponent implements OnInit {

  // allType = {
  //   diagnosis : 'diagnosis',
  //   service: 'service'
  // }

  //@Input() type = this.allType.diagnosis; // 'diagnosis': chẩn đoán ,'service': dịch vụ
  @Input() tags = [];
  dataSource = [];
  searchUpdatePopOver = new Subject<string>();
  mytags: any;
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
      this.mytags = mytags;
      popover.open({ mytags });
    }
  }

  
  getPageDiagnosis(q?: string){
    var val = new ToothDiagnosisPaged();
    val.limit = 10;
    val.offset = 0;
    val.search = q || '';
    this.toothDiagnosisService.getPaged(val).subscribe(result => {
      this.dataSource = result.items;
    })
  }

  loadPopOver(q?: string) {
    this.getPageDiagnosis(q);
  }
  update(tags) {
    this.popover.close();
    this.onSave.emit(tags);
  }

  public valueNormalizer = (text$: Observable<string>): any => text$.pipe(
    switchMap((text: string) => {
      // Search in values
      const matchingValue: any = this.mytags.find((item: any) => {
        // Search for matching item to avoid duplicates
        return item['name'].toLowerCase() === text.toLowerCase();
      });

      if (matchingValue) {
        // Return the already selected matching value and the component will remove it
        return of(matchingValue);
      }

      // Search in data
      const matchingItem: any = this.dataSource.find((item: any) => {
        return item['name'].toLowerCase() === text.toLowerCase();
      });

      if (matchingItem) {
        return of(matchingItem);
      } else {
        
        return of(text).pipe(switchMap(this.service$));
      }
    })
  )

  public service$ = (text: string): any => {
    var val = new ToothDiagnosisSave();
    val.productIds = [];
    val.name = text;
    return this.toothDiagnosisService.create(val).pipe(
      map((result: any) => {
        console.log(result);
        
        return {
          id: result.id,
          name: result.name
        }
      })
    );
      
  }
}
