import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SaleOrderImageComponent } from './sale-order-image.component';

describe('SaleOrderImageComponent', () => {
  let component: SaleOrderImageComponent;
  let fixture: ComponentFixture<SaleOrderImageComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SaleOrderImageComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SaleOrderImageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
