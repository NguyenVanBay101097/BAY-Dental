import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderLineCuComponent } from './sale-order-line-cu.component';

describe('SaleOrderLineCuComponent', () => {
  let component: SaleOrderLineCuComponent;
  let fixture: ComponentFixture<SaleOrderLineCuComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderLineCuComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderLineCuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
