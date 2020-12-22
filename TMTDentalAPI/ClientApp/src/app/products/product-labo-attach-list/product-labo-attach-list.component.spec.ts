import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductLaboAttachListComponent } from './product-labo-attach-list.component';

describe('ProductLaboAttachListComponent', () => {
  let component: ProductLaboAttachListComponent;
  let fixture: ComponentFixture<ProductLaboAttachListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProductLaboAttachListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductLaboAttachListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
