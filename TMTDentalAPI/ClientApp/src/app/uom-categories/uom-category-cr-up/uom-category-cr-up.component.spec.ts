import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UomCategoryCrUpComponent } from './uom-category-cr-up.component';

describe('UomCategoryCrUpComponent', () => {
  let component: UomCategoryCrUpComponent;
  let fixture: ComponentFixture<UomCategoryCrUpComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UomCategoryCrUpComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UomCategoryCrUpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
