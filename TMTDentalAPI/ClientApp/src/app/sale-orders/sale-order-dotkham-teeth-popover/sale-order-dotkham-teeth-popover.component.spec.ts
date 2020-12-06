import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderDotkhamTeethPopoverComponent } from './sale-order-dotkham-teeth-popover.component';

describe('SaleOrderDotkhamTeethPopoverComponent', () => {
  let component: SaleOrderDotkhamTeethPopoverComponent;
  let fixture: ComponentFixture<SaleOrderDotkhamTeethPopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderDotkhamTeethPopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderDotkhamTeethPopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
