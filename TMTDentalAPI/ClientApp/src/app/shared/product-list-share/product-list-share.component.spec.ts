import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductListShareComponent } from './product-list-share.component';

describe('ProductListShareComponent', () => {
  let component: ProductListShareComponent;
  let fixture: ComponentFixture<ProductListShareComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductListShareComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductListShareComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
