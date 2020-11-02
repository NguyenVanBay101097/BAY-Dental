import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderTeethPopoverComponent } from './sale-order-teeth-popover.component';

describe('SaleOrderTeethPopoverComponent', () => {
  let component: SaleOrderTeethPopoverComponent;
  let fixture: ComponentFixture<SaleOrderTeethPopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderTeethPopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderTeethPopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
