import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderLineInfoPopoverComponent } from './sale-order-line-info-popover.component';

describe('SaleOrderLineInfoPopoverComponent', () => {
  let component: SaleOrderLineInfoPopoverComponent;
  let fixture: ComponentFixture<SaleOrderLineInfoPopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderLineInfoPopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderLineInfoPopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
