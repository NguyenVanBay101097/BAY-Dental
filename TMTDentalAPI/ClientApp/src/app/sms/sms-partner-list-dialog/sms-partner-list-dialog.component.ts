import { Component, OnInit } from "@angular/core";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import { GridDataResult } from "@progress/kendo-angular-grid";
import { Subject } from "rxjs";
import { debounceTime, distinctUntilChanged } from "rxjs/operators";
import { PartnerPaged } from "src/app/partners/partner-simple";
import { PartnerService } from "src/app/partners/partner.service";

@Component({
  selector: "app-sms-partner-list-dialog",
  templateUrl: "./sms-partner-list-dialog.component.html",
  styleUrls: ["./sms-partner-list-dialog.component.css"],
})
export class SmsPartnerListDialogComponent implements OnInit {
  gridData: GridDataResult;
  loading = false;
  offset = 0;
  title: string;
  search: string;
  searchUpdate = new Subject<string>();
  selectedIds: any = [];

  constructor(
    public activeModal: NgbActiveModal,
    private partnerService: PartnerService
  ) { }

  ngOnInit() {
    this.loadDataFromApi();

    this.searchUpdate
      .pipe(debounceTime(400), distinctUntilChanged())
      .subscribe((value) => {
        this.loadDataFromApi();
      });
  }

  loadDataFromApi() {
    this.loading = true;
    var val = new PartnerPaged();
    val.search = this.search || "";
    val.customer = true;
    this.partnerService.getCustomerBirthDay(val).subscribe(
      (res: any[]) => {
        this.gridData = {
          data: res,
          total: res.length
        }
        this.loading = false;
      },
      (err) => {
        console.log(err);
        this.loading = false;
      }
    );
  }

  pageChange(event) {
    this.offset = event.skip;
    this.loadDataFromApi();
  }

  onSave() {
    this.activeModal.close(this.selectedIds);
  }
}
