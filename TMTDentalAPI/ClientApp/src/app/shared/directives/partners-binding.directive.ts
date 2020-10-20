import { Directive, Input, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { DataBindingDirective, GridComponent } from '@progress/kendo-angular-grid';
import { Subscription } from 'rxjs';
import { PartnersService } from '../services/partners.service';

@Directive({
    selector: '[partnersBinding]'
})
export class PartnersBindingDirective extends DataBindingDirective implements OnInit, OnDestroy {
    private serviceSubscription: Subscription;
    @Input() advanceFilter: any;
    constructor(private partnersService: PartnersService, grid: GridComponent) {
        super(grid);
    }

    public ngOnInit(): void {
        this.serviceSubscription = this.partnersService.subscribe((result) => {
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
        this.partnersService.getView(this.state, this.advanceFilter);
    }
}
