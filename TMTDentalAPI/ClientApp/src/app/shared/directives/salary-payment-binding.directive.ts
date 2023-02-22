import { Directive, Input, OnDestroy, OnInit } from '@angular/core';
import { DataBindingDirective, GridComponent } from '@progress/kendo-angular-grid';
import { Subscription } from 'rxjs';
import { SalaryPaymentService } from '../services/salary-payment.service';

@Directive({
    selector: '[salaryPaymentBinding]'
})
export class SalaryPaymentBindingDirective extends DataBindingDirective implements OnInit, OnDestroy {
    private serviceSubscription: Subscription;
    @Input() advanceFilter: any;
    constructor(private salaryPaymentService: SalaryPaymentService, grid: GridComponent) {
        super(grid);
    }

    public ngOnInit(): void {
        this.serviceSubscription = this.salaryPaymentService.subscribe((result) => {
            this.grid.loading = false;  // hide the loading indicator
            this.grid.data = result;
            this.notifyDataChange(); // notify the grid that its data has changed
        });

        super.ngOnInit(); // do not forget to call the base implementation

        this.rebind();
    }

    public ngOnDestroy(): void {
        if (this.serviceSubscription) {
            this.serviceSubscription.unsubscribe();
        }

        super.ngOnDestroy();
    }

    public rebind(): void {
        this.grid.loading = true;
        this.salaryPaymentService.getView(this.state, this.advanceFilter);
    }
}
