import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StockInventoryCriteriaCuDialogComponent } from './stock-inventory-criteria-cu-dialog.component';

describe('StockInventoryCriteriaCuDialogComponent', () => {
  let component: StockInventoryCriteriaCuDialogComponent;
  let fixture: ComponentFixture<StockInventoryCriteriaCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StockInventoryCriteriaCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StockInventoryCriteriaCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
