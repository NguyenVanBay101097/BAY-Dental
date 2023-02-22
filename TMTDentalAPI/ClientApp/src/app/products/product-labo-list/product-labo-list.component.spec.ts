import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductLaboListComponent } from './product-labo-list.component';

describe('ProductLaboListComponent', () => {
  let component: ProductLaboListComponent;
  let fixture: ComponentFixture<ProductLaboListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductLaboListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductLaboListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
