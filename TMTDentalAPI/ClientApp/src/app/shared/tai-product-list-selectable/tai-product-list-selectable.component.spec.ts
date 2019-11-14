import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TaiProductListSelectableComponent } from './tai-product-list-selectable.component';

describe('TaiProductListSelectableComponent', () => {
  let component: TaiProductListSelectableComponent;
  let fixture: ComponentFixture<TaiProductListSelectableComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TaiProductListSelectableComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TaiProductListSelectableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
