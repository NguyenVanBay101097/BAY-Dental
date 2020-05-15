import { Component, OnInit } from '@angular/core';
import { UoMDisplay, UomService, UoMPaged, UoMBasic } from 'src/app/uoms/uom.service';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-shared-demo-data-dialog',
  templateUrl: './shared-demo-data-dialog.component.html',
  styleUrls: ['./shared-demo-data-dialog.component.css']
})
export class SharedDemoDataDialogComponent implements OnInit {
  productId: string;
  listUoMs: UoMBasic[] = [];
  limit = 20;
  offset = 0;
  title: string;
  searchUpdate = new Subject<string>();
  search: string;
  constructor(
    private uomService: UomService,
    private activeModal: NgbActiveModal
  ) { }

  ngOnInit() {
    this.searchUpdate.pipe(
      debounceTime(400),
      distinctUntilChanged())
      .subscribe(() => {
        this.loadRecord();
      });

    this.loadRecord();
  }

  loadRecord() {
    this.searchUoMs(this.search).subscribe(
      result => {
        this.listUoMs = result;
        console.log(result);

      }
    )
  }

  chooseUoM(uom) {
    this.uomService.get(uom.id).subscribe(
      result => {
        result.id = uom.id;
        this.activeModal.close(result);
      }
    )
  }

  searchUoMs(q?: string) {
    var uomPaged = new UoMPaged();
    uomPaged.limit = this.limit;
    uomPaged.offset = this.offset;
    uomPaged.productId = this.productId;
    uomPaged.search = q;
    return this.uomService.autocomplete(uomPaged);
  }

}
