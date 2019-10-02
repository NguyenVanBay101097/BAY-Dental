import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, FormArray, Validators } from '@angular/forms';
import { ComboBoxComponent } from '@progress/kendo-angular-dropdowns';
import { WindowRef, WindowService, WindowCloseResult } from '@progress/kendo-angular-dialog';
import { RoutingSimple, RoutingService, RoutingPaged } from 'src/app/routings/routing.service';
import { debounceTime, tap, switchMap } from 'rxjs/operators';
import { DotKhamLineService, DotKhamLineChangeRouting } from 'src/app/dot-khams/dot-kham-line.service';

@Component({
  selector: 'app-dot-kham-line-change-routing-dialog',
  templateUrl: './dot-kham-line-change-routing-dialog.component.html',
  styleUrls: ['./dot-kham-line-change-routing-dialog.component.css']
})
export class DotKhamLineChangeRoutingDialogComponent implements OnInit {
  changeRoutingForm: FormGroup;
  dotKhamLineId: string;
  productId: string;
  filteredRoutings: RoutingSimple[];
  @ViewChild('routingCbx', { static: true }) routingCbx: ComboBoxComponent;

  constructor(private fb: FormBuilder, private windowRef: WindowRef, private routingService: RoutingService,
    private dotKhamLineService: DotKhamLineService) { }

  ngOnInit() {
    this.changeRoutingForm = this.fb.group({
      routing: [null, Validators.required],
    });

    if (this.routingCbx) {
      this.routingCbx.filterChange.asObservable().pipe(
        debounceTime(300),
        tap(() => (this.routingCbx.loading = true)),
        switchMap(value => this.searchRoutings(value))
      ).subscribe(result => {
        this.filteredRoutings = result;
        this.routingCbx.loading = false;
      });

      setTimeout(() => {
        this.routingCbx.focus();
      }, 200);
    }

    this.loadFilteredRoutings();
  }

  loadFilteredRoutings() {
    this.searchRoutings().subscribe(result => this.filteredRoutings = result);
  }

  searchRoutings(search?: string) {
    var val = new RoutingPaged();
    val.productId = this.productId;
    val.search = search;

    return this.routingService.autocompleteSimple(val);
  }

  onSave() {
    if (!this.changeRoutingForm.valid) {
      return;
    }
    var val = new DotKhamLineChangeRouting();
    val.id = this.dotKhamLineId;
    val.routingId = this.changeRoutingForm.get('routing').value.id;
    this.dotKhamLineService.changeRouting(val).subscribe(result => {
      this.windowRef.close(true);
    });
  }

  onCancel() {
    this.windowRef.close();
  }
}



