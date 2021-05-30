import { Component, EventEmitter, Input, OnInit, Output, ViewChild } from '@angular/core';
import { MultiSelectComponent } from '@progress/kendo-angular-dropdowns';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { ProductPaged, ProductService } from 'src/app/products/product.service';

@Component({
  selector: 'app-consulting-service-popover',
  templateUrl: './consulting-service-popover.component.html',
  styleUrls: ['./consulting-service-popover.component.css']
})
export class ConsultingServicePopoverComponent implements OnInit {
  @Input() tags = [];
  dataSource = [];
  searchUpdatePopOver = new Subject<string>();
  @Input() popOverPlace = 'right';
  @Output() onSave = new EventEmitter();
  @ViewChild('popOver', { static: true }) public popover: any;
  @ViewChild('multiSelect', { static: true }) multiSelect: MultiSelectComponent;
  constructor(
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

  getPageProduct(q?:string){
    var val = new ProductPaged();
    val.limit = 0;
    val.offset = 0;
    val.search = q || '';
    val.type2 = 'service';
    this.productService.getPaged(val).subscribe(result => {
      this.dataSource = result.items;
    })
  }

  loadPopOver(q?: string) {
    this.getPageProduct(q);
  }
  update(tags) {
    this.popover.close();
    this.onSave.emit(tags);
  }

}
