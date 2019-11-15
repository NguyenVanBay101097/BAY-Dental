import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TaiSearchInputComponent } from './tai-search-input.component';

describe('TaiSearchInputComponent', () => {
  let component: TaiSearchInputComponent;
  let fixture: ComponentFixture<TaiSearchInputComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TaiSearchInputComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TaiSearchInputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
