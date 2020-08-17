import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderLineDialogComponent } from './sale-order-line-dialog.component';

describe('SaleOrderLineDialogComponent', () => {
  let component: SaleOrderLineDialogComponent;
  let fixture: ComponentFixture<SaleOrderLineDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderLineDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderLineDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
