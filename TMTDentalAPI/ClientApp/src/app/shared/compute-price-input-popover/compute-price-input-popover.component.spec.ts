import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ComputePriceInputPopoverComponent } from './compute-price-input-popover.component';

describe('ComputePriceInputPopoverComponent', () => {
  let component: ComputePriceInputPopoverComponent;
  let fixture: ComponentFixture<ComputePriceInputPopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ComputePriceInputPopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ComputePriceInputPopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
